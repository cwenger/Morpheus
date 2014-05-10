[ -d "$1/.svn" ] && sed "s/\\\$WCREV\\\$/$(svnversion | sed "s/[^0-9]*//g")/g" Properties/AssemblyInfo.template.cs > Properties/AssemblyInfo.cs
