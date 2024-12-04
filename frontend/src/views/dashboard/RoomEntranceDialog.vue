<template>
  <v-dialog
    v-model="displayed"
    max-width="600"
  >
    <v-card>
      <v-card-title>Войти в комнату</v-card-title>
      <v-card-text style="padding-bottom: 0">
        <v-text-field
          v-model="roomId"
          label="ID комнаты"
        />
      </v-card-text>
      <v-card-actions>
        <v-btn
          text="Войти"
          @click="$emit('join', roomId)"
        />
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: "RoomEntranceDialog",
  props: {
    modelValue: Boolean,
  },
  emits: ["update:modelValue", "join"],
  data() {
    return {
      roomId: "" as string,
    };
  },
  computed: {
    displayed: {
      get() {
        return this.modelValue;
      },
      set(value) {
        this.$emit("update:modelValue", value);
      }
    },
  },
});
</script>