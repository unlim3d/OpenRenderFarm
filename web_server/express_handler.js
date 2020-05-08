const fs = require('fs');

const config = require('./config');
const utils = require('./utils');

const ReadRenderedFiles = async function (req, res){
    const frame_format = 'jpg';
    const json_format = 'json';
    const video_format = 'mov';

    const postfix_length = 4;

    //
    let files = await utils.GetFiles(config.files_path);
    if (files[0]){
        res.render(config.views_path + 'main', {files: JSON.stringify(files)});
        console.error(files[1]);
        return files;
    }
    files = files[1];

    const raw_frames = files.filter(x => x.includes('.' + frame_format));
    const raw_jsons = files.filter(x => x.includes('.' + json_format));
    const raw_videos = files.filter(x => x.includes('.' + video_format));
    let raw_result = [];

    const stats_promises = [];
    for (let i = 0; i < raw_frames.length; i++){
        let file = raw_frames[i];
        const wof = file.substr(0, file.length - (frame_format.length + 1));
        const info = {};
        info.type = 'frame';
        info.sequence = wof.substr(0, wof.length - (postfix_length));
        info.num = parseInt(wof.substr(wof.length - postfix_length), 10);
        if (isNaN(info.num)) continue;
        info.format = frame_format;
        info.filename = wof;
        raw_result.push(info);
        stats_promises.push(utils.GetFileStats(config.files_path + file));
    }
    const frames_stats = await Promise.all(stats_promises);
    for (let i = 0; i < raw_result.length; i++){
        if (frames_stats[i] && !frames_stats[i][0]){
            raw_result[i].stats = frames_stats[i][1];
        }
    }
    raw_result = raw_result.sort((a, b) => b.stats.mtime - a.stats.mtime);

    for (let i = 0; i < raw_jsons.length; i++){
        const file = raw_jsons[i];
        let data = await utils.ReadFile(config.files_path + file);
        if (data[0]) continue;
        data = data[1];
        try {
            data = JSON.parse(data);
        }catch (e) {
            console.error('Wrong JSON format at ' + file + '.');
            continue;
        }
        const filename = file.substr(0, file.length - (json_format.length + 1));
        const file_data = raw_result.find(x => x.filename === filename);
        if (file_data){
            file_data.data = data;
        }
    }

    for (let i = 0; i < raw_videos.length; i++){
        const file = raw_videos[i];
        const info = {};
        info.type = 'video';
        const wof = file.substr(0, file.length - (video_format.length + 1));
        info.sequence = wof;
        info.format = video_format;
        info.filename = wof;
        raw_result.push(info);
    }

    const result = [];
    result[0] = [];
    for (let i = 0; i < raw_result.length; i++){
        if (!result[0].includes(raw_result[i].sequence)) result[0].push(raw_result[i].sequence);
        delete raw_result[i].stats;
    }

    result[1] = [];
    for (let i = 0; i < result[0].length; i++){
        result[1][i] = raw_result.find(x => x.sequence === result[0][i] && x.type === 'video');
    }

    const frames = raw_result.filter(x => x.type === 'frame');
    const seq_frames = {};
    const seq_params = {};
    let max_frames = 0;
    for (let i = 0; i < frames.length; i++){
        if (!seq_frames.hasOwnProperty(frames[i].sequence)){
            seq_frames[frames[i].sequence] = frames.filter(x => x.sequence === frames[i].sequence).sort((a, b) => b.num - a.num);

            seq_params[frames[i].sequence] = {};
            seq_params[frames[i].sequence].real_min = seq_frames[frames[i].sequence][seq_frames[frames[i].sequence].length - 1].num;
            seq_params[frames[i].sequence].max = seq_frames[frames[i].sequence][0].num - seq_params[frames[i].sequence].real_min;
            seq_params[frames[i].sequence].min = 0;

            max_frames = Math.max(max_frames, seq_params[frames[i].sequence].max);

            seq_frames[frames[i].sequence] = seq_frames[frames[i].sequence].reduce((acc, cur) => {acc[cur.num - seq_params[frames[i].sequence].real_min] = cur; return acc;}, {});
        }
    }

    for (let i = 0; i <= max_frames; i++){
        result[i + 2] = [];
        for (let j = 0; j < result[0].length; j++){
            if (i > (seq_params[result[0][j]] || {max: -1}).max) continue;
            result[i + 2][j] = seq_frames[result[0][j]][i] || {type: 'skipped'};
        }
    }

    res.render(config.views_path + 'main', {files: JSON.stringify([0, result])});
    return [0, result];
};

const SendFile = async function(req, res){
    const read_stream = fs.createReadStream(config.files_path + req.query.filename);
    if (read_stream){
        read_stream.pipe(res);
    }else{
        res.send('No such file.');
    }
};

const routes = {
    "/": ReadRenderedFiles,
    "/files": SendFile,
};

module.exports = function (route) {
    const handler = routes[route];
    if (!handler){
        return (req, res) => res.send('Wrong Address.');
    }

    return handler;
};