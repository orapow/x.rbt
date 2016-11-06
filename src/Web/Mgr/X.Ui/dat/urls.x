^/$->/app.ashx?v=index&t=1
^(/(css|img|js|um|plug)\S+)$->/res/{0}
^/([\w/]+)-?([\S]*)[.]html$->/app.ashx?v={0}&t=1&p={1}
^/(api)/([\d\w.=]+)$->/app.ashx?v={1}&t=2