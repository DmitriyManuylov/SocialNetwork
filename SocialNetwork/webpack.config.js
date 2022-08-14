const path = require("path");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
    entry: "../wwwroot/ts",
    output: {
        path: path.resolve(__dirname, "wwwroot"),
        filename: "[name].[chunkhash].js",
        publicPath: "/",
    },
    resolve: {
        extensions: [".js", ".ts"],
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                use: "ts-loader",
            },
            {
                test: /\.css$/,
                use: [MiniCssExtractPlugin.loader, "css-loader"],
            },
        ],
    },
    plugins: [
        new CleanWebpackPlugin(),
        new HtmlWebpackPlugin({
            template: "./src/index.html",
        }),
        new MiniCssExtractPlugin({
            filename: "css/[name].[chunkhash].css",
        }),
    ],
};
//"use strict"
//{
//    // Требуется для формирования полного output пути
//    let path = require('path');

//    // Плагин для очистки выходной папки (bundle) перед созданием новой
//    const CleanWebpackPlugin = require('clean-webpack-plugin');

//    // Путь к выходной папке
//    const bundleFolder = "wwwroot/bundle/";

//    module.exports = {
//        // Точка входа в приложение
//        entry: "./Scripts/main.ts",

//        // Выходной файл
//        output: {
//            filename: 'script.js',
//            path: path.resolve(__dirname, bundleFolder)
//        },
//        module: {
//            rules: [
//                {
//                    test: /\.tsx?$/,
//                    loader: "ts-loader",
//                    exclude: /node_modules/,
//                },
//            ]
//        },
//        resolve: {
//            extensions: [".tsx", ".ts", ".js"]
//        },
//        plugins: [
//            new CleanWebpackPlugin([bundleFolder])
//        ],
//        // Включаем генерацию отладочной информации внутри выходного файла
//        // (Нужно для работы отладки клиентских скриптов)
//        devtool: "inline-source-map"
//    };
//}