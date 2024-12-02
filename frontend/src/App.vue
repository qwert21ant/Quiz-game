<template>
  <v-app>
    <RouterView />
    <v-snackbar
      v-model="showSnackbar"
      timeout="3000"
    >
      <div class="d-flex flex-row align-center text-white" style="height: 30px">
        <v-icon
          class="mr-3"
          icon="mdi-alert-circle"
          color="warning"
        />
        <div>{{ error }}</div>
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
import errorBus from "@/utils/ErrorBus"

export default defineComponent({
  name: "App",
  data() {
    return {
      error: "" as string,
      showSnackbar: false as boolean,
    };
  },
  mounted() {
    errorBus.on(err => {
      this.showSnackbar = true;
      this.error = err;
    });
  }
});
</script>
<style>
</style>