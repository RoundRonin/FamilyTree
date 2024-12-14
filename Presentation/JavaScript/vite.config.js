import { resolve } from 'path'
import { defineConfig } from 'vite'

export default defineConfig({
    define: {
        'process.env.NODE_ENV': '"production"'
    },
    build: {
        minify: 'esbuild',
        lib: {
            entry: resolve(__dirname, 'src/FamilyTreeBlazor.Components.ts'),
            name: 'FamilyTreeBlazor.Components',
            fileName: 'FamilyTreeBlazor.Components',
        },
        rollupOptions: {
            output: {
                manualChunks: undefined,
                inlineDynamicImports: true,
                entryFileNames: '[name].js',
                assetFileNames: '[name].[ext]',
            },
        }
    },
})
