!macro NSIS_HOOK_POSTINSTALL
  CreateShortCut "$SMSTARTUP\Just Clipboard Manager.lnk" "$INSTDIR\just-clipboard-manager.exe"
!macroend

!macro NSIS_HOOK_POSTUNINSTALL
  Delete "$SMSTARTUP\Just Clipboard Manager.lnk"
!macroend
