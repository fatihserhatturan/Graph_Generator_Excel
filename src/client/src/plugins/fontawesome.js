// src/plugins/fontawesome.js
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import {
    faMagnifyingGlassPlus,
    faMagnifyingGlassMinus,
    faMaximize,
    faUserGroup,
    faRoad,
    faHandshakeSimple,
    faChevronDown,
    faXmark,
    faList,
    faShareNodes
} from '@fortawesome/free-solid-svg-icons'

// İkonları kütüphaneye ekle
library.add(
    faMagnifyingGlassPlus,  // zoom-in yerine
    faMagnifyingGlassMinus, // zoom-out yerine
    faMaximize,            // expand yerine
    faUserGroup,           // users yerine
    faRoad,               // route yerine
    faHandshakeSimple,    // handshake yerine
    faChevronDown,
    faXmark,              // times yerine
    faList,
    faShareNodes
)

export { FontAwesomeIcon }
