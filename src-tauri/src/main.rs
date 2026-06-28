// Prevents additional console window on Windows in release, DO NOT REMOVE!!
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

use sentry::{protocol::IpAddress, types::Dsn, User};
use std::str::FromStr;

use just_clipboard_manager_lib::config::app_config::AppConfiguration;

fn main() {
  let app_config = AppConfiguration::new().unwrap();

  let _guard = sentry::init(sentry::ClientOptions {
    dsn: Dsn::from_str(&app_config.sentry.dsn).ok(),
    environment: Some(if cfg!(dev) {
      "development".into()
    } else {
      "production".into()
    }),
    release: sentry::release_name!(),
    auto_session_tracking: true,
    ..sentry::ClientOptions::default()
  });
  sentry::configure_scope(|scope| {
    scope.set_user(Some(User {
      id: machine_uid::get().ok(),
      ip_address: Some(IpAddress::Auto),
      ..User::default()
    }));
  });

  just_clipboard_manager_lib::run(app_config);
}
