export default [
  'strapi::errors',
  {
    name: 'strapi::security',
    config: {
      contentSecurityPolicy: {
        useDefaults: true,
        directives: {
          'connect-src': ["'self'", 'http:'],
          'img-src': ["'self'", 'data:', 'blob:', 'cms-notrycatch.obs.ru-moscow-1.hc.sbercloud.ru'],
          'media-src': ["'self'", 'data:', 'blob:', 'cms-notrycatch.obs.ru-moscow-1.hc.sbercloud.ru'],
          upgradeInsecureRequests: null,
        },
      },
    },
  },
  'strapi::cors',
  'strapi::poweredBy',
  'strapi::logger',
  'strapi::query',
  'strapi::body',
  'strapi::session',
  'strapi::favicon',
  'strapi::public',

];
