const path = require("path");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");

module.exports = {
    entry: {
        SocialNetwork: "./Scripts/SocialNetwork.ts",
        UserPage: "./Scripts/UserPage.ts",
        App: "./Scripts/App.ts"
    },
  output: {
    path: path.resolve(__dirname, "wwwroot/dist"),
    filename: "[name].js",
    publicPath: "/",
    },
    optimization: {
        usedExports: 'global',
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
    ],
  },
  plugins: [
    new CleanWebpackPlugin(),
  ],
};
