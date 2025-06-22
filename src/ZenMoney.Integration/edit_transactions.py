import time

import requests
import json

# update this from the website
COOKIE = "PHPSESSID=njkv8l77l0ah940t3pb0qoih37; _ga=GA1.2.1983459596.1698219072; _ym_uid=1698219072671102771; __utmc=180328751; __utmz=180328751.1698219093.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); _ga_Z1Z1XNZELK=GS1.2.1722778499.7.0.1722778499.0.0.0; _ym_d=1739477874; _ym_isad=2; __utma=180328751.1983459596.1698219072.1739477875.1740931728.33"

HOST_URL = "https://zenmoney.ru"
API_URL = f"{HOST_URL}/api/v2"
REFERRER_URL = f"{HOST_URL}/a/"

HEADERS = {
    "accept": "application/json, text/javascript, */*; q=0.01",
    "accept-language": "en-US,en;q=0.9,ru;q=0.8",
    "priority": "u=1, i",
    "sec-ch-ua": "\"Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"133\", \"Chromium\";v=\"133\"",
    "sec-ch-ua-mobile": "?0",
    "sec-ch-ua-platform": "\"Linux\"",
    "sec-fetch-dest": "empty",
    "sec-fetch-mode": "cors",
    "sec-fetch-site": "same-origin",
    "x-requested-with": "XMLHttpRequest",
    "cookie": COOKIE,
    "Referer": REFERRER_URL,
}
HEADERS_POST = {
    **HEADERS,
    "content-type": "application/x-www-form-urlencoded",
}

REFERRER_POLICY = "strict-origin-when-cross-origin"


def fetch_transactions(limit, skip, payee):
    """Fetches a batch of transactions based on limit and skip."""
    url = f"{API_URL}/transaction/?limit={limit}&skip={skip}&type_notlike=uit&payee%5B%5D={payee}&finder="
    print(url)
    response = requests.get(
        url,
        headers=HEADERS,
    )
    for _ in range(3):
        try:
            response.raise_for_status()
            break  # Exit loop if successful
        except requests.exceptions.HTTPError as e:
            if response.status_code == 404:
                print(f"HTTPError: {e}. Retrying...")
                time.sleep(15)
            else:
                raise  # Re-raise the exception for non-404 errors
    return response.json()


def update_transaction(transaction_id, payload):
    """Updates a specific transaction with a POST call."""
    url = f"{API_URL}/transaction/{transaction_id}/"
    response = requests.post(
        url,
        headers=HEADERS_POST,
        data=json.dumps(payload),
    )
    response.raise_for_status()
    return response.json()


def find_and_update_all_transactions(old_payee, new_payee, limit=40):
    skip = 0
    while True:
        # Fetch the current batch of transactions
        transactions = fetch_transactions(limit=limit, skip=skip, payee=old_payee)
        transaction_count = len(transactions)
        print(f"Fetched {transaction_count} transactions: {transactions}")

        if not transactions:
            print("No more transactions to process.")
            break
        for transaction in transactions:
            # Prepare payload for POST request (customize as needed)
            transaction_id = transaction['id']
            payload = {
                "category": "0",
                "tag_groups": ["13126654"],
                "date": transaction["date"],
                "comment": "",
                "payee": new_payee,
                "outcome": transaction['outcome'],
                "income": 0,
                "account_income": transaction["account_income"],
                "account_outcome": transaction["account_outcome"],
                "merchant": "",
                "id": transaction_id
            }
            # Update each transaction
            try:
                result = update_transaction(transaction_id, payload)
                print(f"Successfully updated transaction id {transaction_id}: {result}")
            except requests.exceptions.RequestException as e:
                print(f"Failed to update transaction id {transaction_id}: {e}")

        if transaction_count < limit:
            print("Nothing more to process.")
            break

        # Increment for the next batch
        skip += limit


if __name__ == "__main__":
    find_and_update_all_transactions(old_payee="Uber Eats", new_payee="Uber")
