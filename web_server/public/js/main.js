const CopyToClipboard = function(text) {
    const element = document.createElement('div');
    element.innerText = text;
    document.body.appendChild(element);
    const range = document.createRange();
    range.selectNode(element);
    window.getSelection().removeAllRanges(); // clear current selection
    window.getSelection().addRange(range); // to select text
    document.execCommand("copy");
    window.getSelection().removeAllRanges();// to deselect
    document.body.removeChild(element);
};

const BuildPage = function () {
    let files = JSON.parse(document.getElementById('files').textContent);
    let sequences_info = JSON.parse(document.getElementById('sequences_info').textContent);
    if (!sequences_info[0]) sequences_info = sequences_info[1];
    else sequences_info = {};

    const table = document.getElementById('table');

    if (!files[0] && files[1][0].length > 0){
        files = files[1];
        const ids = files[0];
        let tr = document.createElement('tr');
        for (let i = 0; i < ids.length; i++){
            const th = document.createElement('th');
            th.innerText = (sequences_info[ids[i]] || {}).render_name_mask || "ID: " + ids[i];
            tr.appendChild(th);
        }
        table.appendChild(tr);

        const videos = files[1];
        tr = document.createElement('tr');
        for (let i = 0; i < videos.length; i++){
            const file = videos[i];

            const td = document.createElement('td');
            if (file) {
                td.setAttribute('alt', file.filename);
                td.setAttribute('title', file.filename);
                const video = document.createElement('video');
                video.setAttribute('controls', 'controls');
                video.setAttribute('loop', 'loop');
                const source = document.createElement('source');
                source.setAttribute('src', '/files?filename=' + file.filename + '.' + file.format);
                video.appendChild(source);
                td.appendChild(video);
            }

            const sequence_info = sequences_info[ids[i]];
            if (sequence_info) {
                if (sequence_info.hasOwnProperty('full_renders_size')) {
                    const render_size_info = document.createElement('div');
                    render_size_info.setAttribute('class', 'render_info');
                    render_size_info.innerText = "Full Renders Size: " + (sequence_info.full_renders_size / 1024 / 1024 / 1024).toLocaleString(undefined, {maximumFractionDigits: 2}) + "GB.";
                    td.appendChild(render_size_info);
                }

                if (sequence_info.hasOwnProperty('render_path')) {
                    const render_path = document.createElement('div');
                    render_path.setAttribute('class', 'render_info');
                    render_path.innerText = "Render Path: " + sequence_info.render_path;
                    td.appendChild(render_path);
                }
            }

            const path_field = document.createElement('input');
            path_field.setAttribute('type', 'text');
            td.appendChild(path_field);

            const button_save = document.createElement('button');
            button_save.setAttribute('class', 'save_btn');
            button_save.innerText = 'Set save path';
            button_save.onclick = () => {
                const body = {sequence: ids[i]};
                if (path_field.value) body.path = path_field.value;
                MakeProgramRequest({name: 'SetRenderFolder', body: body});
            };
            td.appendChild(button_save);

            if (sequence_info) {
                if (sequence_info.hasOwnProperty('frames_missed') && sequence_info.frames_missed.length > 0) {
                    const missed_btn = document.createElement('button');
                    missed_btn.setAttribute('class', 'missed_btn');
                    missed_btn.innerText = 'Copy missed frames: ' + sequence_info.frames_missed.length;
                    if (sequence_info.hasOwnProperty('min_frame_rendered') && sequence_info.hasOwnProperty('max_frame_rendered')){
                        missed_btn.innerText += " (" + (sequence_info.frames_missed.length / (sequence_info.max_frame_rendered - sequence_info.min_frame_rendered) * 100).toLocaleString(undefined, {maximumFractionDigits: 2}) + "%)";
                    }
                    missed_btn.onclick = () => {
                        const clipboard = sequence_info.frames_missed.reduce((acc, cur) => acc === '' ? cur : cur + ',' + acc, '');
                        CopyToClipboard(clipboard);
                    };
                    td.appendChild(missed_btn);
                }
            }

            tr.appendChild(td);
            table.appendChild(tr);
        }

        for (let i = 2; i < files.length; i++) {
            const tr = document.createElement('tr');
            for (let j = 0; j < files[i].length; j++){
                const file = files[i][j];

                const td = document.createElement('td');
                const img = document.createElement('img');

                td.appendChild(img);
                if (file && file.type === 'frame') {
                    td.setAttribute('alt', file.filename);
                    td.setAttribute('title', file.filename);

                    img.setAttribute('src', '/files?filename=' + file.filename + '.' + file.format);

                    if (file.data){
                        const fields = ['Slave', 'Renderer', 'Frame', 'RenderTime', 'ServerPreviewFileName'];

                        for (let k = 0; k < fields.length; k++){
                            const val = file.data[fields[k]];
                            const div = document.createElement('div');
                            div.setAttribute('class', 'frame_info');
                            div.innerText = fields[k] + ': ' + val;
                            div.setAttribute('style', 'top: ' + ((k + .5) * 1.5) + 'em');
                            td.appendChild(div);
                        }
                    }
                }else if (file && file.type === 'skipped'){
                    img.setAttribute('src', 'img/no_preview.svg');
                }

                if (!file || !file.data){
                    const div = document.createElement('div');
                    div.setAttribute('class', 'frame_info');
                    div.innerText = 'No Such Info';
                    div.setAttribute('style', 'top: 1.5em');
                    td.appendChild(div);
                }

                tr.appendChild(td);
                table.appendChild(tr);
            }
        }
    }else{
        const div_no_files = document.createElement('div');
        div_no_files.setAttribute('class', 'attention no_files');
        div_no_files.innerText = 'You don\'t have any rendered files.';
        table.parentElement.appendChild(div_no_files);
    }
};

const Wait = function (timeout) {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve();
        }, timeout);
    });
};

const MakeProgramRequest = async function(options){
    const address = 'http://127.0.0.1:8090';
    const xhr = new XMLHttpRequest();

    options.method = options.method || 'POST';
    options.name = options.name || 'SetRenderFolder';
    options.body = options.body || {};

    xhr.onreadystatechange = () => {
        if (xhr.readyState === 4 && xhr.status === 200) {
            alert(xhr.response);
        }
    };

    xhr.open(options.method, address + '/' + options.name);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send(JSON.stringify(options.body));
};

$(document).ready(async function() {
    BuildPage();
});