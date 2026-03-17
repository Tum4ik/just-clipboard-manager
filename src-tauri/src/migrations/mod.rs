mod v1;
mod v2;

use tauri_plugin_sql::Migration;

pub fn migrations() -> Vec<Migration> {
  vec![v1::v1(), v2::v2()]
}
