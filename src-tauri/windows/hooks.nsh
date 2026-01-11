!macro NSIS_HOOK_POSTINSTALL
  CreateShortCut "$SMSTARTUP\${PRODUCT_NAME}.lnk" "$INSTDIR\${BINARY_NAME}.exe"
!macroend

!macro NSIS_HOOK_POSTUNINSTALL
  Delete "$SMSTARTUP\${PRODUCT_NAME}.lnk"
!macroend
