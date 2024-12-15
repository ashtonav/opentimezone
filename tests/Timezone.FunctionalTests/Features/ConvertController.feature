Feature: Timezone Conversion
As an API user,
I want to ensure that the timezone conversion API is accurate
So that I can reliably convert date and time across different timezones

    Scenario Outline: Requesting to convert time from one timezone to another timezone should return the correct time
        Given I have a request to convert date '<DateTime>' from '<FromTimezone>' to '<ToTimezone>'
        When I send the request
        Then the response should return '200' status code
        And the response should correctly convert the time into '<ExpectedDateTime>'

        Examples:
          | DateTime               | FromTimezone       | ToTimezone         | ExpectedDateTime       |
          | "2023-02-01T12:00:00" | "UTC"              | "America/New_York" | "2023-02-01T07:00:00" |
          | "2023-02-01T07:00:00" | "America/New_York" | "UTC"              | "2023-02-01T12:00:00" |
          | "2023-02-01T07:00:00" | "America/New_York" | "Europe/London"    | "2023-02-01T12:00:00" |
          | "2023-02-01T07:00:00" | "America/New_York" | "Asia/Tokyo"       | "2023-02-01T21:00:00" |

