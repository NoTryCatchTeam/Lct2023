export default [
  'strapi::errors',
  'strapi::security',
  'strapi::cors',
  'strapi::poweredBy',
  'strapi::logger',
  'strapi::query',
  'strapi::session',
  'strapi::favicon',
  'strapi::public',
  {
    name: "strapi::body",
    config: {
      formLimit: "1024mb", // modify form body
      jsonLimit: "1024mb", // modify JSON body
      textLimit: "1024mb", // modify text body
      formidable: {
        maxFileSize: 1000 * 1024 * 1024, // multipart data, modify here limit of uploaded file size
      },
    },
  },
];
