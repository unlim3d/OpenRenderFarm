const fs = require('fs');
const path = require('path');
const mime = require('mime-types');

const config = require('./config');
const utils = require('./utils');

const ReadRenderedFiles = async function (req, res){
    const frame_format = 'jpg';
    const json_format = 'json';
    const video_format = 'mov';

    const postfix_length = 4;

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
        let wof = file.substr(0, file.length - (frame_format.length + 1));
        const info = {};
        info.type = 'frame';
        info.id = wof.split('_')[0];
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
        info.id = wof.split('_')[0];
        info.format = video_format;
        info.filename = wof;
        raw_result.push(info);
    }

    const result = [];
    result[0] = [];
    for (let i = 0; i < raw_result.length; i++){
        if (!result[0].includes(raw_result[i].id)) result[0].push(raw_result[i].id);
        delete raw_result[i].stats;
    }

    result[1] = [];
    for (let i = 0; i < result[0].length; i++){
        result[1][i] = raw_result.find(x => x.id === result[0][i] && x.type === 'video');
    }

    const frames = raw_result.filter(x => x.type === 'frame');
    const seq_frames = {};
    const seq_params = {};
    let max_frames = 0;
    for (let i = 0; i < frames.length; i++){
        if (!seq_frames.hasOwnProperty(frames[i].id)){
            seq_frames[frames[i].id] = frames.filter(x => x.id === frames[i].id).sort((a, b) => b.num - a.num);

            seq_params[frames[i].id] = {};
            seq_params[frames[i].id].real_min = seq_frames[frames[i].id][seq_frames[frames[i].id].length - 1].num;
            seq_params[frames[i].id].max = seq_frames[frames[i].id][0].num - seq_params[frames[i].id].real_min;
            seq_params[frames[i].id].min = 0;

            max_frames = Math.max(max_frames, seq_params[frames[i].id].max);

            seq_frames[frames[i].id] = seq_frames[frames[i].id].reduce((acc, cur) => {acc[cur.num - seq_params[frames[i].id].real_min] = cur; return acc;}, {});
        }
    }

    for (let i = 0; i <= max_frames; i++){
        result[i + 2] = [];
        for (let j = 0; j < result[0].length; j++){
            if (i > (seq_params[result[0][j]] || {max: -1}).max) continue;
            result[i + 2][j] = seq_frames[result[0][j]][i] || {type: 'skipped'};
        }
    }

    const jobs_path = path.join(config.files_path, './Jobs');
    files = await utils.GetFiles(jobs_path);
    if (files[0]){
        res.render(config.views_path + 'main', {files: JSON.stringify([0, result]), sequences_info: JSON.stringify(files)});
        console.error(files[1]);
        return files;
    }
    files = files[1];

    let info_json_files = files.filter(x => x.includes('.' + json_format));

    const sequences_info = {};
    for (let i = 0; i < info_json_files.length; i++){
        const file = info_json_files[i];
        let data = await utils.ReadFile(path.join(jobs_path, file));
        if (data[0]) continue;
        data = data[1];
        try {
            data = JSON.parse(data);
        }catch (e) {
            console.error('Wrong JSON format at ' + file + '.');
            continue;
        }

        const id = file.split('_')[0];
        sequences_info[id] = {
            render_name_mask: data.RenderNameMask,
            render_path: data.RenderPath,
            full_renders_size: data.FullRendersSize,
            frame_size: data.OneFrameSize,
            frames_missed: data.FramesMissed,
            min_frame_rendered: data.MinimumFrameRendered,
            max_frame_rendered: data.MaximumFrameRendered,
            file_format: data.FileFormat,
        };
    }

    res.render(config.views_path + 'main', {files: JSON.stringify([0, result]), sequences_info: JSON.stringify([0, sequences_info])});
    return [0, result];
};

const Dir = async function(req, res){
    let result = await utils.GetFiles(config.files_path + req.query.path);
    res.json(result);
};

const SendFile = async function(req, res){
    const full_path = config.files_path + req.query.filename;
    let read_stream = fs.createReadStream(full_path);
    read_stream.on('error', () => {
        res.removeHeader('Content-Disposition');
        res.removeHeader('Content-Type');
        res.status(404);
        res.send('No such file.');
        read_stream.close();
    });
    if (read_stream){
        res.header('Content-Disposition', 'attachment; filename="' + path.basename(req.query.filename) + '"');
        res.header('Content-Type', mime.lookup(full_path));
        read_stream.pipe(res);
    }else{
        res.status(404);
        res.send('No such file.');
    }
};

const routes = {
    "/": ReadRenderedFiles,
    "/files": SendFile,
    "/dir": Dir,
};

module.exports = function (route) {
    const handler = routes[route];
    if (!handler){
        return (req, res) => res.send('Wrong Address.');
    }

    return handler;
};