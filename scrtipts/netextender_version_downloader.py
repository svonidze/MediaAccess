import requests
import os
import threading

base_url = "https://software.sonicwall.com/NetExtender/"

file_name_pattern = "NetExtender-linux-amd64-10.2.236-XXX.deb"


# NetExtender-linux-amd64-10.3.0-21.deb - exists!
# file_name_pattern = "NetExtender-linux-amd64-10.3.0-XXX.deb"


def check_and_download_file(number):
    file_name = file_name_pattern.replace("XXX", str(number))
    url = base_url + file_name
    print(f"Checking: {url}")

    try:
        response = requests.head(url)
        response_status_code = response.status_code
        if response_status_code == 200:
            print(f"File found: {url}")

            file_response = requests.get(url)

            downloads_folder = os.path.join(os.path.expanduser("~"), "Downloads")
            file_path = os.path.join(downloads_folder, file_name)

            print(F"Downloading file to '{file_path}'")
            with open(file_path, "wb") as file:
                file.write(file_response.content)

            print("File downloaded successfully!")
            return True
        else:
            print(f"File not found, HTTP status code: {response_status_code}. {response}")
    except requests.RequestException as e:
        print(f"Error checking URL {url}: {e}")

    return False


threads = []
for i in range(10000, 1000, -1):
    thread = threading.Thread(target=check_and_download_file, args=(i,))
    threads.append(thread)
    thread.start()

for thread in threads:
    while thread.is_alive():
        thread.join(timeout=0.1)
