const fs = require('fs');

const ReadFile = function (path) {
    return new Promise((resolve) => {
        fs.readFile(path, 'utf8', (err, data) => {
            if (err) resolve([1, err]);
            resolve([0, data]);
        });
    });
};

const GetFileStats = function (path){
    return new Promise(resolve => {
        fs.stat(path, (err, stats) => {
            if (err){


                fs.mkdir(path); //Create dir in case not found
                resolve([1, err]);
                console.error(err);
                return;
            }

            resolve([0, stats]);
        });
    });
};

const GetFiles = function (path){
    return new Promise(resolve => {
        fs.readdir(path, async function (err, files) {
            if (err){
                resolve([1, err]);
            }

            resolve([0, files]);
        });
    });
};

module.exports = {
    ReadFile: ReadFile,
    GetFileStats: GetFileStats,
    GetFiles: GetFiles,
}