export default ({ env }) => {
    // ...
   const email = {
      config: {
        provider: 'nodemailer',
        providerOptions: {
          host: env('SMTP_HOST', 'imap.mail.ru'),
          port: env('SMTP_PORT', 993),
          auth: {
            user: env('SMTP_USERNAME'),
            pass: env('SMTP_PASSWORD'),
          },
          // ... any custom nodemailer options
        },
        settings: {
          defaultFrom: 'ltc2023@notrycatch.team',
          defaultReplyTo: 'ltc2023@notrycatch.team',
        },
      },
    }
  };