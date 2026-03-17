Feature: Installed Plugins
  As a user
  I want to review, enable/disable and uninstall installed plugins
  So that I can manage which plugins are active and remove those I no longer need

  Background:
    Given the Main window is activated
    When the user clicks the "Plugins" top-level tab button
    And the user clicks the "Installed" tab button of Plugins view

  Scenario: Text plugin is installed by default
    Then the plugin "Text plugin" must be installed
    And the plugin "Text plugin" must be enabled
