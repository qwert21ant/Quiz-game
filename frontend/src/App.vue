<template>
  <v-app>
    <AppBar />
    <RouterView />
    <v-snackbar
      v-model="showSnackbar"
      timeout="3000"
    >
      <div class="d-flex flex-row align-center text-white" style="height: 30px">
        <v-icon
          v-if="message?.type !== 'info'"
          class="mr-3"
          icon="mdi-alert-circle"
          :color="message?.type == 'info' ? 'white' : 'warning'"
        />
        <div>{{ message.content }}</div>
        <v-btn
          class="ml-auto"
          icon="mdi-window-close"
          variant="plain"
          color="white"
          @click="showSnackbar = false"
        />
      </div>
    </v-snackbar>
  </v-app>
</template>
<script lang="ts">
import { defineComponent } from 'vue';
import messageBus, { Message } from "@/utils/MessageBus"
import AppBar from './views/common/AppBar.vue';

export default defineComponent({
  name: "App",
  components: { AppBar },
  data() {
    return {
      message: null as Message,
      showSnackbar: false as boolean,
    };
  },
  computed: {
    snackbarIcon() {
      if (!this.message) return "";
      if (this.message.type === "error")
        return "mdi-alert-circle";
    },
    snackbarColor() {
      if (!this.message) return "";
    },
  },
  mounted() {
    messageBus.on(msg => {
      this.message = msg;
      this.showSnackbar = true;
    });
  }
});
</script>
<style>
</style>