mod v1;

use tauri_plugin_sql::Migration;

pub fn migrations() -> Vec<Migration> {
  vec![v1::v1()]
}
