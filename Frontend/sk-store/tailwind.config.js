/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: '#2a4365',
        secondary: '#c53030',
        accent: '#ecc94b',
      },
      fontFamily: {
        'playfair': ['"Playfair Display"', 'serif'],
        'roboto': ['Roboto', 'sans-serif'],
      },
    },
  },
  plugins: [],
} 