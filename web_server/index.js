const express = require('express');
const bodyParser = require('body-parser');
const cookieParser = require('cookie-parser');
const express_handler = require('./express_handler');

// noinspection JSPotentiallyInvalidConstructorUsage
const app = new express();

app.set('view engine', 'ejs');

app.use(express.static(__dirname + '/public'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({extended: true}));
app.use(cookieParser());

// noinspection JSUnresolvedFunction
app.get('*', function (req, res) {
    express_handler(req.url.split('?')[0])(req, res);
});

app.listen(8089);