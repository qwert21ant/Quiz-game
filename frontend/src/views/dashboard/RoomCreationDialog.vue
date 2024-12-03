<template>
  <v-dialog
    v-model="displayed"
    max-width="600"
  >
    <v-card>
      <v-card-title>Создать комнату</v-card-title>
      <v-card-text>
        <v-text-field
          label="Имя комнаты"
        />
        <v-select></v-select>
      </v-card-text>
      <v-card-actions>
        <v-btn
          text="Создать"
        />
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: "RoomCreationDialog",
  props: {
    modelValue: Boolean,
  },
  emits: ["update:modelValue"],
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