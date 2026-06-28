use config::{Config, ConfigError, File};
use serde::Deserialize;
use std::{env, path::PathBuf};

#[derive(Debug, Deserialize)]
#[serde(rename_all = "kebab-case")]
pub struct AppConfiguration {
  pub database: Database,
  pub sentry: Sentry,
}

#[derive(Debug, Deserialize)]
#[serde(rename_all = "kebab-case")]
pub struct Database {
  pub connection_string: String,
}

#[derive(Debug, Deserialize)]
#[serde(rename_all = "kebab-case")]
pub struct Sentry {
  pub dsn: String,
}

impl AppConfiguration {
  pub fn new() -> Result<Self, ConfigError> {
    let exe_dir = env::current_exe()
      .ok()
      .and_then(|p| p.parent().map(|p| p.to_path_buf()))
      .unwrap_or_else(|| PathBuf::from("."));
    let config = Config::builder()
      .add_source(File::from(exe_dir.join("config/default")))
      .add_source(File::from(exe_dir.join("config/development")).required(false))
      .build()?;
    config.try_deserialize()
  }
}
