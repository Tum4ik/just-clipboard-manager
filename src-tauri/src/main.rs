// Prevents additional console window on Windows in release, DO NOT REMOVE!!
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

use config::{Config, File};
use machine_uid;
use sentry::{types::Dsn, User};
use std::str::FromStr;

fn main() {
  let config = Config::builder()
    .add_source(File::with_name("config/default"))
    .add_source(File::with_name("config/development").required(false))
    .build()
    .expect("Failed to load config");

  let sentry_dsn = config.get::<String>("sentry.dsn").unwrap();
  let sentry_environment = config.get::<String>("sentry.environment").unwrap();

  let _guard = sentry::init(sentry::ClientOptions {
    dsn: Dsn::from_str(&sentry_dsn).ok(),
    environment: Some(sentry_environment.into()),
    release: sentry::release_name!(),
    auto_session_tracking: true,
    ..sentry::ClientOptions::default()
  });
  sentry::configure_scope(|scope| {
    scope.set_user(Some(User {
      id: machine_uid::get().ok(),
      ..User::default()
    }));
  });

  just_clipboard_manager_lib::run();
}
