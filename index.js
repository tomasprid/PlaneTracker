const path = require('path');
const request = require('request');
const cheerio = require('cheerio');
const http = require('http');
const fs = require('fs');
const url = require('url');
const querystring = require('querystring');
const extract = require('extract-zip')
const rimraf = require('rimraf');

//const gotZip = require('got-zip');

const config = {
    downloadUrl: 'http://www.histdata.com/get.php',
    baseUrl: 'http://www.histdata.com/download-free-forex-historical-data/?/ascii/tick-data-quotes/',
    pairs: [   'eurusd', 'ukxgbp', 'usddkk', 'usdsgd', 'xagusd', 'xaugbp'],
    years: ['2018', '2019'],
    months: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
    dir: path.join(__dirname),
};

function getAllPairs() {
    return new Promise((resolve, reject) => {
        http.get(config.baseUrl, (resp) => {
            let data = '';
            resp.on('data', (chunk) => {
                data += chunk;
            });
            resp.on('end', () => {
                const $ = cheerio.load(data);
                const pairs = $('#content-wrapper table td strong').get().map(i => $(i).text());
                resolve(pairs.map(raw => raw.toLowerCase().replace('/', '')));
            });
        }).on("error", (err) => {
            reject(err);
        });
    })
}

function extractZip(filename, pair) {
    return new Promise((resolve, reject) => {
        extract(path.join(__dirname, 'temp', filename), {dir: path.join(__dirname, 'temp', 'extracted')}, (err) => {
            if (err) {
                reject(err);
            } else {
                resolve();
            }
        });
    }).then(() => {
        const files = fs.readdirSync(path.join(__dirname, 'temp', 'extracted'));
        const csvFilename = files.filter(file => file.indexOf('.csv') !== -1)[0];
        if (!fs.existsSync(path.join(__dirname, 'out', pair))) {
            fs.mkdirSync(path.join(__dirname, 'out', pair));
        }
        fs.copyFileSync(path.join(__dirname, 'temp', 'extracted', csvFilename), path.join(__dirname, 'out', pair, csvFilename));
        return rmdir(path.join(__dirname, 'temp', 'extracted'));
    }).catch((e) => {
        console.log('Cannot unpack ZIP')
    });
}

function downloadSingleZip (pair, year, month) {
    const downloadPage = config.baseUrl + pair.toLowerCase() + '/' + year + '/' + month;
    return new Promise((resolve, reject) => {
        //first download the form data
        http.get(downloadPage, (resp) => {
            let data = '';

            // A chunk of data has been recieved.
            resp.on('data', (chunk) => {
                data += chunk;
            });

            // The whole response has been received. Print out the result.
            resp.on('end', () => {
                const $ = cheerio.load(data);
                const formData = {};
                $('#file_down input').get().forEach((item) => {
                    const name = $(item).attr('name');
                    const value = $(item).attr('value');
                    formData[name] = value;
                });
                resolve(formData);
            });

        }).on("error", (err) => {
            reject(err);
        });
    }).then((formData) => {
        return new Promise((resolve, reject) => {

            const data = querystring.stringify(formData);
            const parsedUrl = url.parse(config.downloadUrl);
            const options = {
                host: parsedUrl.host,
                path: parsedUrl.pathname,
                method: 'POST',
                protocol: parsedUrl.protocol,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'Content-Length': Buffer.byteLength(data),
                    Referer: downloadPage,
                }
            };

            console.log(downloadPage)
            const name = year.concat(month) + '.zip';
            const file = fs.createWriteStream(path.join(__dirname, 'temp/'.concat(pair), name));
            const request = http.request(options, (res) => {
                res.pipe(file);
                res.on('end', () => {
                    resolve(name);
                    file.close();
                });
            });

            request.write(data);
            request.end();
        });
    })
    .catch((err) => {
        console.warn('Error', err);
    });
}

function rmdir(path) {
    return new Promise((resolve, reject) => {
        rimraf(path, (e) => {
            if (e) {
                reject(e);
            } else {
                resolve();
            }
        })
    });
}

async function start() {
    await rmdir('temp');
    await rmdir('out');
    fs.mkdirSync('temp');
    fs.mkdirSync('out');

    //config.pairs = await getAllPairs();
    console.log('Processing pairs:', config.pairs);
    for (const pair of config.pairs) {
		fs.mkdirSync('temp/'.concat(pair));
        for (const year of config.years) {
	    for (const month of config.months) {
		
                await downloadSingleZip(pair, year, month);
            }
	}
    }
}

start();

/*
function callback(error, response, body) {
	if (!error && response.statusCode == 200) {
		console.log('ok');
	}
}

request(options, callback);
*/