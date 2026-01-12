!macro NSIS_HOOK_POSTINSTALL
  CreateShortCut "$SMSTARTUP\${PRODUCTNAME}.lnk" "$INSTDIR\${MAINBINARYNAME}.exe"
!macroend

!macro NSIS_HOOK_POSTUNINSTALL
  Delete "$SMSTARTUP\${PRODUCTNAME}.lnk"
!macroend
