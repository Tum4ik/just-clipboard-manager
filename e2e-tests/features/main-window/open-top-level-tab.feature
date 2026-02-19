Feature: Open top level tab of the Main window
  To test if specific top-level tabs of the Main window open correctly

  Scenario: "Settings" tab is open by default
    Given Settings top-level tab button
    Then Settings tab must be selected
