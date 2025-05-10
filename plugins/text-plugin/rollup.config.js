import commonjs from '@rollup/plugin-commonjs';
import json from '@rollup/plugin-json';
import { nodeResolve } from '@rollup/plugin-node-resolve';
import typescript from '@rollup/plugin-typescript';

export default {
  input: 'index.ts',
  output: {
    file: '../../src-tauri/resources/plugins/text-plugin/plugin-bundle.mjs',
    format: 'esm'
  },
  plugins: [
    nodeResolve(),
    commonjs(),
    typescript(),
    json(),
  ],
};
