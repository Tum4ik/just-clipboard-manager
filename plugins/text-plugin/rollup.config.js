import commonjs from '@rollup/plugin-commonjs';
import { nodeResolve } from '@rollup/plugin-node-resolve';
import typescript from '@rollup/plugin-typescript';

export default {
  input: 'index.ts',
  output: {
    file: './dist/plugins/text-plugin/plugin-bundle.mjs',
    format: 'esm'
  },
  plugins: [
    nodeResolve(),
    commonjs(),
    typescript(),
  ],
};
