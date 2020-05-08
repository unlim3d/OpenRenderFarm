const utils = require('./base/utils');

const Start = async function (){
    const promises = [];
    promises.push(await utils.Exec('npm list express || npm install express --save'));
    promises.push(await utils.Exec('npm list cookie-parser|| npm install cookie-parser --save'));
    promises.push(await utils.Exec('npm list ejs || npm install ejs --save'));
    promises.push(await utils.Exec('npm list body-parser || npm install body-parser --save'));
    promises.push(await utils.Exec('npm list @types/node || npm install @types/node --save'));
    await Promise.all(promises);
    console.log("Checking modules Finished.");
    const routing = require('./index');
};

Start();