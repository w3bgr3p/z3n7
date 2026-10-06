# Mail

Mailboxes for registrations and one-time codes. Each client stores the mailbox it works with in project variables (`email`, `mailId` and its own id variable) so that the next call finds it.

| Type | Service |
|---|---|
| [[AnyMessage]] | AnyMessage: short-term and long-term mailboxes on public domains. |
| [[BestMailBox]], [[z3nmail]] | Temporary mailboxes on own domains (mail.autoz3n.xyz). |
| [[TempMail]] | Temp Mail (Privatix) on RapidAPI. |
| [[FirstMail]] | FirstMail: one mailbox that receives mail forwarded from other addresses. |
| [[GmailClient]] | Gmail over the Gmail API (OAuth refresh token). |
| [[MSMail]] | Outlook/Hotmail over Microsoft Graph (OAuth refresh token). |

```csharp
var mail = new BestMailBox(project);
var box = mail.NewMail();                 // [id, email]; email also in project.Profile.Email
// ... submit the form with box[1] ...
string code = mail.Otp(deadline: 90);
```

API: [[API reference#Mail|Mail]]. Source: [`Mail/`](https://github.com/w3bgr3p/z3n7/tree/master/z3n7/Mail)
