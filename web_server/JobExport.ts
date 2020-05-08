import fs = require('fs');
import * as path from "path";

module.exports = function (RenderPath: string )
{
    let data = JSON.stringify(RenderPath);
    fs.writeFileSync( path.join(RenderPath,".json"), data);
    console.log("ExportJob: "+RenderPath);
}