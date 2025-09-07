#!/usr/bin/env python3
"""
CaixaBank file fetch utility

This script reproduces the POST request shown in the issue and then attempts to
retrieve the resulting file. It uses only Python standard library (urllib).

Default values are taken from the issue description. You can override them from
CLI arguments. Provide your own Cookie header and WebLogicSession if needed.

Examples:
  python3 src/caixa_bank.py \
    --weblogic-session "HKUlpDv2MSwKSj9CQuEGFlFnjgTc_LTxFeotTN6qcmsA-02embw-!653907297!973543869" \
    --contract 961318951308030 \
    --start 20250901 --end 20250907 --format E \
    --cookie-file cookies.txt \
    --output movimientos.xls

You can place the entire Cookie header value into a file (cookies.txt), or pass
it directly via --cookie. The script sets a reasonable set of headers including
User-Agent, Accept-Language, etc., but you can extend or change them with
--extra-header if necessary.
"""
from __future__ import annotations

import argparse
import os
import re
import sys
import urllib.parse
import urllib.request
from typing import Dict, Optional, Tuple

DEFAULT_HOST = "https://loc8.caixabank.es"
DEFAULT_PATH = "/GPeticiones"
DEFAULT_QUERY = {
    "PN": "TJD",
    "PE": "2",
    "CLICK_ORIG": "FLX_TJD_1",
}
DEFAULT_USER_AGENT = (
    "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 "
    "(KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36"
)

# Known binary content types that we will treat as files to save
BINARY_CONTENT_TYPES = {
    "application/vnd.ms-excel",
    "application/pdf",
    "application/octet-stream",
    "application/zip",
    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
}


def parse_kv_header_pairs(pairs: Optional[list[str]]) -> Dict[str, str]:
    headers: Dict[str, str] = {}
    if not pairs:
        return headers
    for p in pairs:
        if ":" not in p:
            raise argparse.ArgumentTypeError(
                f"Invalid header format: {p!r}. Use 'Name: Value'"
            )
        name, value = p.split(":", 1)
        headers[name.strip()] = value.strip()
    return headers


def load_cookie_from_file(path: str) -> str:
    with open(path, "r", encoding="utf-8") as f:
        # Just read the whole file; treat it as already formatted Cookie header value
        return f.read().strip()


def build_url(weblogic_session: str, query: Dict[str, str]) -> str:
    # URL form: /GPeticiones;WebLogicSession=<session>?PN=...&PE=...&CLICK_ORIG=...
    session_path = f"{DEFAULT_PATH};WebLogicSession={weblogic_session}"
    return f"{DEFAULT_HOST}{session_path}?{urllib.parse.urlencode(query)}"


def build_request(url: str, form: Dict[str, str], headers: Dict[str, str]) -> urllib.request.Request:
    data = urllib.parse.urlencode(form).encode("utf-8")
    req = urllib.request.Request(url=url, data=data, method="POST")
    for k, v in headers.items():
        req.add_header(k, v)
    # Ensure mandatory headers we care about
    if "User-Agent" not in headers:
        req.add_header("User-Agent", DEFAULT_USER_AGENT)
    if "Accept" not in headers:
        req.add_header("Accept", "*/*")
    if "Content-Type" not in headers:
        req.add_header("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8")
    return req


def pick_output_filename(disposition: Optional[str], default_name: str) -> str:
    if not disposition:
        return default_name
    # Try to parse filename from Content-Disposition
    # e.g. attachment; filename="movimientos.xls"
    m = re.search(r'filename\*=UTF-8''([^;\r\n]+)', disposition)
    if m:
        return urllib.parse.unquote(m.group(1))
    m2 = re.search(r'filename\s*=\s*"?([^";\r\n]+)"?', disposition)
    if m2:
        return m2.group(1)
    return default_name


def is_binary_content(content_type: Optional[str]) -> bool:
    if not content_type:
        return False
    ct = content_type.split(";")[0].strip().lower()
    return ct in BINARY_CONTENT_TYPES


def extract_first_url_from_text(text: str) -> Optional[str]:
    # Very permissive URL finder
    url_regex = re.compile(r"https?://[^\s'\"]+", re.IGNORECASE)
    m = url_regex.search(text)
    if m:
        return m.group(0)
    return None


def download_url(url: str, headers: Dict[str, str]) -> Tuple[bytes, Dict[str, str]]:
    req = urllib.request.Request(url=url, method="GET")
    for k, v in headers.items():
        if k.lower() == "content-type":
            continue  # not needed for GET
        req.add_header(k, v)
    with urllib.request.urlopen(req) as resp:
        body = resp.read()
        resp_headers = {k: v for k, v in resp.headers.items()}
        return body, resp_headers


def main(argv: Optional[list[str]] = None) -> int:
    ap = argparse.ArgumentParser(description="Fetch statement file from CaixaBank endpoint")

    ap.add_argument("--weblogic-session", required=True, help="WebLogicSession value from the URL")
    ap.add_argument("--contract", default="961318951308030", help="NUMERO_CONTRATO")
    ap.add_argument("--tipo", default="C", help="TIPO_CONTRATO (e.g., C)")
    ap.add_argument("--start", default="20250901", help="FECHA_INICIO in YYYYMMDD")
    ap.add_argument("--end", default="20250907", help="FECHA_FIN in YYYYMMDD")
    ap.add_argument("--format", default="E", choices=["E", "P"], help="FORMATO: E (Excel) or P (PDF)")
    ap.add_argument("--flag-cancelled", default="", help="FLAG_TARJETAS_CANCELADAS value (usually empty)")

    ap.add_argument("--cookie", default=None, help="Cookie header value")
    ap.add_argument("--cookie-file", default=None, help="Path to file with Cookie header value")

    ap.add_argument("--extra-header", action="append", default=None,
                    help="Additional header 'Name: Value'. Can be used multiple times")

    ap.add_argument("--output", default=None, help="Output filename. If absent, use server-suggested name or movimientos.xls/pdf")
    ap.add_argument("--dry-run", action="store_true", help="Build and print the request but do not perform network IO")

    args = ap.parse_args(argv)

    # Prepare cookies
    cookie_value: Optional[str] = args.cookie
    if not cookie_value and args.cookie_file:
        cookie_value = load_cookie_from_file(args.cookie_file)

    # Prepare headers
    headers: Dict[str, str] = {
        "Accept-Language": "en-US,en;q=0.9,ru;q=0.8,es;q=0.7",
        "Connection": "keep-alive",
        "DNT": "1",
        "Origin": DEFAULT_HOST,
        "Referer": f"{DEFAULT_HOST}{DEFAULT_PATH};WebLogicSession={args.weblogic_session}",
        "Sec-Fetch-Dest": "empty",
        "Sec-Fetch-Mode": "cors",
        "Sec-Fetch-Site": "same-origin",
        "User-Agent": DEFAULT_USER_AGENT,
        "X-Requested-With": "XMLHttpRequest",
        "sec-ch-ua": '"Chromium";v="140", "Not=A?Brand";v="24", "Google Chrome";v="140"',
        "sec-ch-ua-mobile": "?0",
        'sec-ch-ua-platform': '"Linux"',
    }
    if cookie_value:
        headers["Cookie"] = cookie_value

    # Extra headers
    headers.update(parse_kv_header_pairs(args.extra_header))

    # Build URL & form data
    url = build_url(args.weblogic_session, DEFAULT_QUERY)
    form = {
        "NUMERO_CONTRATO": str(args.contract),
        "TIPO_CONTRATO": str(args.tipo),
        "FECHA_INICIO": str(args.start),
        "FECHA_FIN": str(args.end),
        "FORMATO": str(args.format),
        "FLAG_TARJETAS_CANCELADAS": str(args.flag_cancelled),
    }

    if args.dry_run:
        print("[DRY RUN] URL:", url)
        print("[DRY RUN] Headers:")
        for k, v in headers.items():
            if k.lower() == "cookie":
                shown = (v[:60] + "...") if len(v) > 60 else v
                print(f"  {k}: {shown}")
            else:
                print(f"  {k}: {v}")
        print("[DRY RUN] Form:", form)
        return 0

    # Perform POST
    req = build_request(url, form, headers)
    try:
        with urllib.request.urlopen(req) as resp:
            post_body: bytes = resp.read()
            resp_headers = {k: v for k, v in resp.headers.items()}
            status = getattr(resp, 'status', None) or resp.getcode()
            final_url = resp.geturl()
    except Exception as e:
        print("Request failed:", e, file=sys.stderr)
        return 2

    print(f"POST status: {status}; final URL: {final_url}")

    # Check if server already returned the file
    content_type = resp_headers.get("Content-Type")
    content_disp = resp_headers.get("Content-Disposition")

    default_name = "movimientos.xls" if args.format == "E" else "movimientos.pdf"
    out_name = args.output or pick_output_filename(content_disp, default_name)

    if is_binary_content(content_type) or content_disp:
        with open(out_name, "wb") as f:
            f.write(post_body)
        print(f"Saved file to {out_name}")
        return 0

    # If not binary, try to parse a URL from the response body
    try:
        post_text = post_body.decode("utf-8", errors="ignore")
    except Exception:
        post_text = ""

    next_url = extract_first_url_from_text(post_text)
    if not next_url:
        # If no URL in body, try downloading from the final URL (GET)
        next_url = final_url

    print(f"Attempting download from: {next_url}")
    try:
        file_bytes, get_headers = download_url(next_url, headers)
    except Exception as e:
        # If GET also fails, save HTML of POST response for inspection
        html_dump = out_name + ".html"
        with open(html_dump, "w", encoding="utf-8") as f:
            f.write(post_text)
        print(f"Download failed: {e}")
        print(f"Saved POST response HTML to {html_dump} for inspection")
        return 3

    get_ct = get_headers.get("Content-Type")
    get_cd = get_headers.get("Content-Disposition")

    final_out = args.output or pick_output_filename(get_cd, default_name)

    if is_binary_content(get_ct) or get_cd or file_bytes:
        with open(final_out, "wb") as f:
            f.write(file_bytes)
        print(f"Saved file to {final_out}")
        return 0

    # Not clearly a file; save as HTML/text for inspection
    html_dump = final_out + ".html"
    try:
        text = file_bytes.decode("utf-8", errors="ignore")
    except Exception:
        text = ""
    with open(html_dump, "w", encoding="utf-8") as f:
        f.write(text)
    print(f"Server did not provide a binary file. Saved response to {html_dump}")
    return 4


if __name__ == "__main__":
    sys.exit(main())
