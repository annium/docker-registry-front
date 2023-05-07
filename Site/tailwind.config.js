/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Private/**/*.{razor,html,cshtml}",
        "./Public/**/*.{razor,html,cshtml}",
        "./Shared/**/*.{razor,html,cshtml}",
    ],
    theme: {
        extend: {},
    },
    plugins: [],
}

