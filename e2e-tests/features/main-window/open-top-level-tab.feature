Feature: Main Window Top-Level Tabs
  As a user
  I want to navigate between top-level tabs in the main window
  So that I can access different sections of the application

  Background:
    Given the Main window is opened

  Scenario: Settings tab is open by default
    Then the "Settings" tab must be selected
    And the "Settings" tab content is displayed

  Scenario Outline: User can switch tabs
    When the user clicks the "<tab>" button
    Then the "<tab>" tab must be selected
    And the "<tab>" tab content is displayed

    Examples:
      | tab      |
      | Plugins  |
      | About    |
      | Settings |
