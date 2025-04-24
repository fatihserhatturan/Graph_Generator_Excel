import { createApp } from 'vue'
import App from './App.vue'
import { FontAwesomeIcon } from './plugins/fontawesome'
import '@fortawesome/fontawesome-svg-core/styles.css'

const app = createApp(App)
app.component('font-awesome-icon', FontAwesomeIcon)
app.mount('#app')
