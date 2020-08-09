# Gmail-Bot
https://www.notion.so/Automated-Email-Checker-c4398d8fedb34096986546b533460ad7
---------

## Description

.Net core console application to process incoming Gmail messages.

## Scenario

When started it executes the following routine and exits:

1. Connect to Gmail
2. Retrieve last N unread messages
3. For each message
    1. If it is not from one of predefined email addresses then skip to the next message
    2. In message text look for predefined Regex patterns
    3. If no pattern instances found then skip to the next message
    4. For each match save first capturing group
    5. Send response that includes original message text and values from the previous step
    6. Mark original message as read
    
## Message processing example

### Original message

```
Hi all!

Hello Alice
Hello Bob
Hi Carol

Thanks, bye!
```

### Regex patterns

- `Hello ([A-z]+)`
- `Hi ([A-z]+)`
- `Good afternoon ([A-z]+)`

### Response

```
Hi,

I see all
I see Alice
I see Bob
I see Carol

Thank you!

— original message —

Hi all!

Hello Alice
Hello Bob
Hi Carol

Thanks, bye!
```

## Configuration

Should be specified in a class with properties:

- Gmail user name
- Password
- Number of messages to retrieve on start
- Array of email addresses to process
- Array of Regex patterns

Configuration values can be hardcoded in Main() method.

## Other

Response text can be hardcoded.

Use NLog for logging.

Push solution to Github or any other public git repository.

## How to apply

Send e-mail to [arthur.stankevich@gmail.com](mailto:arthur.stankevich@gmail.com) with Subject "Email Regex Checker" .
Include:

1. What problems are possible when running this and how they can be prevented and mitigated?
2. Which Nuget packages are you going to use?
3. How much time will it take for you to implement this?
4. What compensation for the job would be perfect for you?
