# OpenTimezone

<img src="docs/logo.svg" alt="Description" width="105">

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=ashtonav_opentimezone&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=ashtonav_opentimezone)
[![license](https://img.shields.io/github/license/ashtonav/opentimezone.svg)](LICENSE)
[![.NET](https://github.com/ashtonav/opentimezone/actions/workflows/timezonewebapi-app.yml/badge.svg)](https://github.com/ashtonav/opentimezone/actions/workflows/timezonewebapi-app.yml)

OpenTimezone is an open source, free, public API that converts time between different time zones.

OpenTimezone provides:
- A free, public API at [api.opentimezone.com](https://api.opentimezone.com).
- Documentation at [opentimezone.com](https://opentimezone.com).
- Self-hosting support (see the [Installation](#installation) section).

## Usage

### Example 1: Convert time from New York to London.

```bash
 curl -X 'POST' \
    'https://api.opentimezone.com/convert' \
    -H 'Content-Type: application/json' \
    -d '{
    "dateTime": "2024-10-14T00:00:00.000",
    "fromTimezone": "America/New_York",
    "toTimezone": "Europe/London"
    }' 
```

[![Try it out](https://img.shields.io/badge/-Try%20it%20out-brightgreen?style=for-the-badge)](https://hoppscotch.io/?method=POST&url=https%3A%2F%2Fapi.opentimezone.com%2Fconvert&bodyMode=raw&contentType=application%2Fjson&rawParams=%7B%22dateTime%22%3A%222024-10-14T00%3A00%3A00.000%22%2C%22fromTimezone%22%3A%22America%2FNew_York%22%2C%22toTimezone%22%3A%22Europe%2FLondon%22%7D)

---

### Example 2: Convert UTC to New York time.

```bash
curl -X 'POST' \
  'https://api.opentimezone.com/convert' \
  -H 'Content-Type: application/json' \
  -d '{
    "dateTime": "2025-09-17T04:36:48.131338",
    "fromTimezone": "UTC",
    "toTimezone": "America/New_York"
  }'
````

[![Try it out](https://img.shields.io/badge/-Try%20it%20out-brightgreen?style=for-the-badge)](https://hoppscotch.io/?method=POST&url=https%3A%2F%2Fapi.opentimezone.com%2Fconvert&bodyMode=raw&contentType=application%2Fjson&rawParams=%7B%22dateTime%22%3A%222025-09-17T04%3A36%3A48.131338%22%2C%22fromTimezone%22%3A%22UTC%22%2C%22toTimezone%22%3A%22America%2FNew_York%22%7D)

---

### Example 3: Convert New York to UTC time.

```bash
curl -X 'POST' \
  'https://api.opentimezone.com/convert' \
  -H 'Content-Type: application/json' \
  -d '{
    "dateTime": "2025-09-17T00:36:48.131394",
    "fromTimezone": "America/New_York",
    "toTimezone": "UTC"
  }'
```

[![Try it out](https://img.shields.io/badge/-Try%20it%20out-brightgreen?style=for-the-badge)](https://hoppscotch.io/?method=POST&url=https%3A%2F%2Fapi.opentimezone.com%2Fconvert&bodyMode=raw&contentType=application%2Fjson&rawParams=%7B%22dateTime%22%3A%222025-09-17T00%3A36%3A48.131394%22%2C%22fromTimezone%22%3A%22America%2FNew_York%22%2C%22toTimezone%22%3A%22UTC%22%7D)

---

### Example 4: List all supported time zones.

```bash
curl -X 'GET' 'https://api.opentimezone.com/timezones'
```

[![Try it out](https://img.shields.io/badge/-Try%20it%20out-brightgreen?style=for-the-badge)](https://hoppscotch.io/?method=GET&url=https%3A%2F%2Fapi.opentimezone.com%2Ftimezones)


---

## Installation

### Option 1: Using Docker (recommended for self-hosting)

#### Requirements
- Docker

#### How to Run
1. From the root folder of the project, run the following commands:
   ```bash
   docker build -t opentimezone -f ./src/Timezone.WebApi/Dockerfile .
   docker run -it -p 5280:8080 opentimezone
   ```
2. The API can be accessed at [http://localhost:5280](http://localhost:5280).

### Option 2: Using Visual Studio (recommended for development purposes)

#### Requirements
- Visual Studio 2026
    - With ASP.NET and web development installed from the Visual Studio Installer
- Docker
- .NET 10 SDK

#### How to Run
1. Open the solution in Visual Studio 2026.
2. Run it using Docker. 
3. The API can be accessed at [https://localhost:5281](https://localhost:5281).

#### How to Test
1. Open the solution in Visual Studio 2026.
2. Run the tests in Test Explorer.

## Contributing

Pull requests are accepted.

## License

[MIT](https://choosealicense.com/licenses/mit/)
