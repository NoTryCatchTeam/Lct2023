import fetch, {
  Blob,
  blobFrom,
  blobFromSync,
  File,
  fileFrom,
  fileFromSync,
} from 'node-fetch'
import cryptoJS from 'crypto-js';
import { Buffer } from 'buffer';
import fs from 'fs';

export = {
  init(providerOptions) {
    // init your provider if necessary

    return {
      async upload(file) {
      },
      async uploadStream(file) {
        const url = 'http://cms-notrycatch.obs.ru-moscow-1.hc.sbercloud.ru/' + file.name;
        const contentMD5 = "";
        const canonicalizedHeaders = "";
        const canonicalizedResource = "/cms-notrycatch/" + file.name;
        const requestTime = new Date().toUTCString();
        const accessKey = 'change'
        const securityKey = 'change'
        const canonicalString = "PUT" + "\n" + contentMD5 + "\n" + file.mime + "\n" + requestTime + "\n" + canonicalizedHeaders + canonicalizedResource;
        const signature = cryptoJS.HmacSHA1(Buffer.from(canonicalString, 'utf-8').toString(), securityKey);
        const readStream = fs.createReadStream(file.stream.path)
        const headers = {
          "Date": requestTime,
          "Authorization": "OBS " + accessKey + ":" + cryptoJS.enc.Base64.stringify(signature),
          "content-type": file.mime,
          "Content-Length": file.size * 1000
        }
        try {
          const response = await fetch(url, { method: 'PUT', body: readStream, headers: headers });
          return await response.text();
        }
        catch (error)
        {
          return error.message;
         // throw new Error(`Error! status: ${error.message}`);
        }
      },
      delete(file) {
        // delete the file in the provider
      },
      checkFileSize(file, { sizeLimit }) {
        // (optional)
        // implement your own file size limit logic
      },
      getSignedUrl(file) {
        // (optional)
        // Generate a signed URL for the given file.
        // The signed URL allows secure access to the file.
        // Only Content Manager assets will be signed.
        // Returns an object {url: string}.
      },
      isPrivate() {
        // (optional)
        // if it is private, file urls will be signed
        // Returns a boolean
      },
    };
  },
};