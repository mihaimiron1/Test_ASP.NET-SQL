module.exports = {
  content: [
    "./Views/**/*.cshtml",
        "./wwwroot/**/*.js",
        './node_modules/flowbite/**/*.js',
        "./node_modules/apexcharts/**/*.js",
        "./wwwroot/js/**/*.js",
  ],
  theme: {
    extend: {},
  },
    plugins: [
        require('flowbite/plugin'),
    ],
}
