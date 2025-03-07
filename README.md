# WebParser

## Description
WebParser is a .NET 9 application that parses web pages using various options to store results in JSON, CSV, and log files. The application also allows specifying URLs for processing and using blacklist files.

## Requirements
- Docker

## Setup and Run Instructions
### Building Docker Image
```sh
docker build -t webparser:latest .
```
### Running the Container
If you need to pass or retrieve files to/from the container, use mounting:
```sh
docker run -v /path/to/host/data:/app/data -d webparser:latest --black-list /app/data/blacklist.json --file-json /app/data/output.json --file-csv /app/data/output.csv --url https://example.com
```
 
1. Create the folder `/path/on/host` and copy the files you want to pass to the application there.
2. Make sure you grant write permissions to the folder `/path/on/host`:
```sh
chmod o+w /path/on/host
```
If file transfer/retrieval is not needed, you can run it without mounting:
```-v /path/to/host/data:/app/data```

### Application Options
You can pass additional parameters to the application at runtime:
- --no-cli: Disables output of logs to the terminal.

- -j, --file-json: Path to save data in `.json` format. Multiple paths can be specified separated by a space.

- -c, --file-csv: Path to save data in `.csv` format. Multiple paths can be specified separated by a space.

- -o, --file-log: Path to save log files. Multiple paths can be specified separated by a space.

- -a, --url-aw: URL link for Amountwork. URL must have a full path (starts with `https://`).

- -u, --url: URL link for other websites. URL must have a full path (starts with `http://` or `https://`).

- -l, --url-list: Path to the file with URL links. The file must contain just a single URL on each line. URL must have a full path (starts with `http://` or `https://`).
Example file content:
```
https://amountwork.com/ua/rabota/ssha/stroitel/
https://amountwork.com/ua/rabota/ssha/prepodavatel_uchitel/
```

- -b, --black-list: Path to the `JSON` file with blacklist collection (a collection of strings).
Example file content:
```
[{"Store"}, {"Road"}]
```

- --a2: Alpha2 code for the country whose mobile numbers to search for.

- --country-code: Path to the `JSON` file with the country codes whose mobile numbers to search for in alpha2 format.
Example file content:
```
[{"US"}, {"GB"}]
```
