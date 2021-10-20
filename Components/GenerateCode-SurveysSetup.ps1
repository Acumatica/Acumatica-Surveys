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
            #endregion //$componentId

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
        #endregion //InitializeSurveyComponents
"@


$AtributeSection = ""
$InitializeSurveyAttributeSection = ""
foreach($file in dir .\Atributes)
{
 

    #Loads the xml file and casts it into an easy to work with object
    [xml]$atribute =  Get-Content $file.FullName
    #[xml]$atribute =  Get-Content C:\GitKraken\Acumatica-Surveys\Components\Atributes\SUHELPRES.xml
    $attributeID = $atribute.'data-set'.data.CSAttribute.row.AttributeID
    $HeaderDescription = $atribute.'data-set'.data.CSAttribute.row.Description
    $ControlType = $atribute.'data-set'.data.CSAttribute.row.ControlType
    $isinternal  = ""
    $noteId = $atribute.'data-set'.data.CSAttribute.row.NoteID

     if($atribute.'data-set'.data.CSAttribute.row.IsInternal -eq "1")
      {
            $isinternal = "true"
      }
      else
      {
            $isinternal = "false"
      }


    $details = $atribute.'data-set'.data.CSAttribute.row.CSAttributeDetail

    $codegenCsAttributeDetails = ""
    foreach($csAtributeDetail in $details)
    {
      $ValueId = $csAtributeDetail.ValueID 
      $Description = $csAtributeDetail.Description 
      $SortOrder = $csAtributeDetail.SortOrder
      $Disabled = ""
      $NoteID = $csAtributeDetail.NoteID

      if($csAtributeDetail.Disabled -eq "1")
      {
            $Disabled = "true"
      }
      else
      {
            $Disabled = "false"
      }

      $codegenCsAttributeDetails += @" 
            new CSAttributeDetail
            {
                ValueID = "$ValueId", 
                Description = "$Description",
                SortOrder = $SortOrder,
                Disabled = $Disabled,
                NoteID = Guid.Parse("$NoteID")
            },
"@
    }
    

    $newAtribute = @"

            #region $attributeID
            public CSAttribute cs$attributeID = new CSAttribute
            {
                AttributeID = "$attributeID",
                Description = "$HeaderDescription", 
                ControlType = $ControlType,
                IsInternal = $isinternal,
                NoteID = Guid.Parse("$noteId")
            };

            public List<CSAttributeDetail> cs$($attributeID)Detail = new List<CSAttributeDetail>
            {
                $codegenCsAttributeDetails
            };
            #endregion //$attributeID

"@
    
    $AtributeSection +=  $newAtribute



    $InitializeSurveyAttributeSection += @"
            InitializeSurveyAttribute(cs$attributeID,cs$($attributeID)Detail);
              
"@

}

$InitializeSurveyAttributeMethod = @"
        #region InitializeSurveyAttributes
        public void InitializeSurveyAttributes()
        {
            $InitializeSurveyAttributeSection
        }
        #endregion //InitializeSurveyAttributes
"@


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
        #endregion //SurveyComponents
        #region Attributes
        $AtributeSection
        $InitializeSurveyAttributeMethod
        #endregion //Attributes
    }
}
"@






$class | clip.exe

#$class
