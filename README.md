# Fritz.StaticWeb

A simple static blog generator built to support the [KlipTok blog](https://kliptok.com/blog). 

It was originally built as a command-line tool that can run against a folder of templates and blog posts in markdown format. 

With the January 2023 updates, it now hosts its own management website tool inside the executable.  This allows construction of a static website using a complete graphical user-interface complete with markdown editor.  This can be accessed by running the application with no arguments or executing it the the single 'run' verb:

`Fritz.StaticWeb run`

The eventual goal is that this repository generates a GitHub action that will be used to generate the KlipTok blog in that repository