# GitCleanup
A command line tool that cleans up old branches in your git repo

##Setup
Build from source or grab the [latest release](https://github.com/mirhagk/GitCleanup/releases) and extract to a folder on your computer.

Run the program while inside of a git repo to run the cleanup there.

The easiest way to use it is to setup a git alias. Something like the following should work (just change the path to wherever you have it installed):

~~~
git config --global alias.cleanup '!C:/tools/gitcleanup/GitCleanup.exe cleanup -trunk dev --SkipPrune'
~~~
