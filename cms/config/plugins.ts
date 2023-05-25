export default ({ env }) => ({
   email : {
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
    },
    upload: {
      config: {
        provider: 'strapi-provider-upload-cloud-s3', // For community providers pass the full package name (e.g. provider: 'strapi-provider-upload-google-cloud-storage')
        providerOptions: {
          accessKeyId: env('AWS_ACCESS_KEY_ID'),
          secretAccessKey: env('AWS_ACCESS_SECRET'),
          region: env('AWS_REGION'),
          params: {
            ACL: env('AWS_ACL', 'public-read'), // 'private' if you want to make the uploaded files private
            Bucket: env('AWS_BUCKET'),
          },
        },
      },
    }
  });