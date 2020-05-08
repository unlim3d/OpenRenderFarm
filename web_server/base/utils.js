const { exec } = require('child_process');

const Exec = function (command) {
    return new Promise(resolve => {
        exec(command, (err, stdout, stderr) =>{
            if (err){
                resolve([1, err.message]);
                return;
            }
            if (stderr){
                resolve([2, stderr]);
                return;
            }
            resolve([0, stdout]);
        });
    });
};

module.exports = {
    Exec: Exec,
};