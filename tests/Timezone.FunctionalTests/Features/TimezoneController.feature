Feature: Timezone Retrieval
As an API user,
I want to be able to interact with the timezone data,
So that I can retrieve all timezones, request specific timezone information, and receive proper error handling for invalid timezone requests.

    Scenario: Requesting all timezones should return all timezones
        Given I have a request to get all timezones
        When I send the request
        Then the response should return '200' status code
        # As of 15th of December 2024, there are 419 timezones.
        Then I should get at least 400 timezones

    Scenario: Request one valid timezone should return one timezone
        Given I have a request to get 'America/New_York' timezone
        When I send the request
        Then the response should return '200' status code
        Then the response should contain timezone information:
        | Name                         | Offset |
        | (UTC-05:00) America/New_York | -5     |

    Scenario: Request one invalid timezone should return 404
        Given I have a request to get 'invalid-timezone' timezone
        When I send the request
        Then the response should return '404' status code
