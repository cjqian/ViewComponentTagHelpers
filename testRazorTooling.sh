cd sample/ViewComponentTagHelper.Web/
dotnet razor-tooling resolve-taghelpers ./project.json ViewComponentTagHelper.Web > vcth.current
DIFF=$(diff vcth.current vcth.expected)

if [ "$DIFF" != "" ]
then
    echo "Test failed."
    echo $DIFF
else
   echo "Test passed!"
fi

