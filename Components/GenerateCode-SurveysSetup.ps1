$entitySection = ""

foreach($file in dir .\ServeyComponets)
{
    #Loads the xml file and casts it into an easy to work with object
    [xml]$xmlServeyComponent =  Get-Content $file.FullName
    $componentId   = $xmlServeyComponent.'data-set'.data.SurveyComponent.row.ComponentID
    $componentType = $xmlServeyComponent.'data-set'.data.SurveyComponent.row.ComponentType
    $active        = $xmlServeyComponent.'data-set'.data.SurveyComponent.row.Active
    $description   = $xmlServeyComponent.'data-set'.data.SurveyComponent.row.Description
    $noteId        = $xmlServeyComponent.'data-set'.data.SurveyComponent.row.NoteID
    #Being that the conversion from XML strips out the special chars for the html of the body
    #we will use a regex to get the value out of the body atribute
    $rawServeyComponent = Get-Content $file.FullName
    $body  = [regex]::match($rawServeyComponent,'Body=\"(.*)\" NoteID=').Groups[1].Value
 
    $newComponent = @"

            #region $componentId
            public SurveyComponent sc$componentId = new SurveyComponent
            {
                ComponentID = "$componentId",
                ComponentType = "$componentType",
                Active = true,
                Description = "$description",
                Body = @"$body",
                NoteID = Guid.Parse("$noteId")
            };
            #endregion

"@
    
    $entitySection +=  $newComponent

}

#$entitySection

$class = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable IdentifierTypo

namespace PX.Survey.Ext
{
    public class SurveyAutoInit
    {

        #region SurveyComponents
        $entitySection
        #endregion
    }
}
"@

$class | clip.exe
