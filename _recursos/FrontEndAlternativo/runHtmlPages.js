// Decidir através do URL do pedido qual o ficheiro a ver.
const http = require('http');
const url = require('url');
const fs = require('fs');
const path = require('path'); 

http.createServer(function (req, res) {
    var q = url.parse(req.url, true);
    var filename = "." + q.pathname;
    fs.readFile(filename, function(err, data) {
        if (err) {
            res.writeHead(404, {'Content-Type': 'text/html'});
            return res.end(`<h1>404 Not Found</h1>
                <p>The HTML file should be in the same directory as the 'nodeFileServer.js' script is!</p>
                <p>The file name should be inputed in the URL. ex.: localhost:8081/the_file.html</p>`);
        }
        res.writeHead(200, {'Content-Type': 'text/html'});
        res.write(data);
        return res.end();
    })
}).listen(8081);

console.log(`> file server started in: http://localhost:8081/`);
fs.readdir(__dirname, { withFileTypes: true }, (err, files) => {
    console.log(`\n> List of possible files:`);
    if (err) 
        console.log(err); 
    else { 
        files.forEach(file => { 
            if (path.extname(file.name) === ".html") 
                console.log(`  - http://localhost:8081/${file.name}`);
        }) 
    }
})
console.log("> to close connection use Ctrl+C ...");