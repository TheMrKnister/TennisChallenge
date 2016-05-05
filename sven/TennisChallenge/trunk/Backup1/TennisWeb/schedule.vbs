Call RunIt()
Sub RunIt()

Dim RequestObj
Dim URL
Set RequestObj = CreateObject("Microsoft.XMLHTTP")

'Request URL...
URL = "https://www.tennis-challenge.ch/PictureCleanup"

'Open request and pass the URL
RequestObj.open "POST", URL , false

'Send Request
RequestObj.Send

'cleanup
Set RequestObj = Nothing
End Sub