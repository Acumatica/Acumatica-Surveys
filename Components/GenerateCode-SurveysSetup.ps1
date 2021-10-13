$entitySection = ""
$InitializeSurveyComponentsSection = ""
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
            #endregion #$componentId

"@
    $entitySection +=  $newComponent

    $InitializeSurveyComponentsSection += @"
            InitializeSurveyComponent(sc$componentId);
              
"@

}

$InitializeSurveyComponentsMethod = @"
        #region InitializeSurveyComponents
        public void InitializeSurveyComponents()
        {
            $InitializeSurveyComponentsSection
        }
        #endregion #InitializeSurveyComponents
"@


$AtributeSection = ""
foreach($file in dir .\Atributes)
{
 

    #Loads the xml file and casts it into an easy to work with object
    [xml]$atribute =  Get-Content $file.FullName
    #[xml]$atribute =  Get-Content C:\GitKraken\Acumatica-Surveys\Components\Atributes\SUHELPRES.xml
    $attributeID = $atribute.'data-set'.data.CSAttribute.row.AttributeID
    $Description = $atribute.'data-set'.data.CSAttribute.row.Description
    $ControlType = $atribute.'data-set'.data.CSAttribute.row.ControlType
    $isinternal  = $atribute.'data-set'.data.CSAttribute.row.IsInternal
    $noteId = $atribute.'data-set'.data.CSAttribute.row.NoteID

    $details = $atribute.'data-set'.data.CSAttribute.row.CSAttributeDetail

    $codegenCsAttributeDetails = ""
    foreach($csAtributeDetail in $details)
    {
      $ValueId = $csAtributeDetail.ValueID 
      $Description = $csAtributeDetail.Description 
      $SortOrder = $csAtributeDetail.SortOrder
      $Disabled = $csAtributeDetail.Disabled
      $NoteID = $csAtributeDetail.NoteID

      $codegenCsAttributeDetails += @" 
            new CSAttributeDetail
            {
                ValueID = "$ValueId", 
                Description = "$Description",
                SortOrder = $SortOrder,
                Disabled = bool.Parse("$Disabled"),
                NoteID = Guid.Parse("$NoteID")
            },
"@
    }
    

    $newAtribute = @"

            #region $attributeID
            public CSAttribute cs$attributeID = new CSAttribute
            {
                AttributeID = "$attributeID",
                Description = "$Description", 
                ControlType = $ControlType,
                IsInternal = bool.Parse("$isinternal"),
                NoteID = Guid.Parse("$noteId")
            };

            public List<CSAttributeDetail> Cs$($attributeID)Detail = new List<CSAttributeDetail>
            {
                $codegenCsAttributeDetails
            };
            #endregion #$attributeID

"@
    
    $AtributeSection +=  $newAtribute

}


#$entitySection

$class = @"
using PX.CS;
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
    public partial class SurveyCustomizationPlugin
    {

        #region SurveyComponents
        $entitySection
        $InitializeSurveyComponentsMethod
        #endregion #SurveyComponents
        #region Attributes
        $AtributeSection
        #endregion #Attributes
    }
}
"@






$class | clip.exe

#$class
